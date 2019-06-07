using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Serilog;
using UmbracoTestData.Models;

namespace UmbracoTestData
{
    internal class DataCreator
    {
        private readonly string _backOfficeUrl;
        private readonly uint _chunkSize;
        private readonly uint _countOfContentNodes;
        private readonly uint _maxItemsPrLevel;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _password;
        private readonly string _user;


        public DataCreator(
            string backOfficeUrl,
            string user,
            string password,
            uint countOfContentNodes,
            uint chunkSize,
            uint maxItemsPrLevel,
            ILogger logger)
        {
            _backOfficeUrl = backOfficeUrl.TrimEnd('/');
            _user = user;
            _password = password;
            _countOfContentNodes = countOfContentNodes;
            _logger = logger;
            _maxItemsPrLevel = maxItemsPrLevel;
            _chunkSize = chunkSize;
            _httpClient = new HttpClient();
        }

        public async Task Start()
        {
            _logger.Information("Login");
            await Login();

            _logger.Information("CreateDocType");
            await CreateDocType();

            _logger.Information("Create content");
            await CreateContent();
        }

        private async Task CreateDocType()
        {
            var key = Guid.Parse("3460ad16-7165-4b3b-a73a-f911ba1b5d5e");

            var model = new DocumentType(key, "Test Data")
            {
                AllowAsRoot = true,
                AllowedContentTypes = new[] { 0 }, // Allow it self as child
                Id = 0,
                AllowCultureVariant = true,
                Groups = new[]
                {
                    new DocumentTypeGroup
                    {
                        Name = "Group 1",
                        Properties = new[]
                        {
                            new DocumentTypeGroupProperty
                            {
                                Alias = "text",
                                Label = "Text",
                                DataTypeId = -88
                            }
                        }
                    }
                }
            };

            var result = await PostJsonAsync(Constants.Paths.CreateDocType, model);

            _logger.Verbose("Create DocType result: {result}", result);
            //TODO handle non happy path
        }

        private async Task CreateContent()
        {
            var runner = _countOfContentNodes;
            var numberOfLevels = 1U;
            while (runner > _maxItemsPrLevel)
            {
                numberOfLevels++;
                runner /= _maxItemsPrLevel;
            }

            _logger.Information(
                "Creating content: {totalCount}. {numberOfLevels} levels with {numberOfItemsOnEachLevel} items in each level.",
                _countOfContentNodes,
                numberOfLevels,
                _maxItemsPrLevel);

            var container = await CreateSingleContent("TestData Container");
            await CreateManyContent(_countOfContentNodes, _maxItemsPrLevel,  container.Id,
                new int[_countOfContentNodes]);

            //TODO handle non happy path
        }

        private async Task CreateManyContent(uint countOfContentNodes,
            uint numberOfItemsOnEachLevel, int parentId, IList<int> parentIds)
        {
            //Build one level at a time in parallel 
            var levels = BuildLevels(countOfContentNodes, numberOfItemsOnEachLevel);


            for (var i = 1; i < 10; i++) // If more then 10 levels, then just quit - That's too insane.
            {
                var current = levels.Where(x => x.Value.Count == i).ToArray();

                if (!current.Any())
                {
                    break;
                }

                _logger.Information("Create level {level} of content. Total of {numberInLevel}", i, current.Length);


                var chunks = current.Chunk(_chunkSize).ToArray();
                for (var j = 0; j < chunks.Length; j++)
                {
                    var chunk = chunks[j];

                    var tasks = new Dictionary<int, Task<Content>>();
                    foreach (var (key, _) in chunk)
                    {
                        var name = GetNameBasedOnPositionInTree((uint) key, numberOfItemsOnEachLevel);
                        var parentIndex = GetParentIndex((uint) key, numberOfItemsOnEachLevel);

                        var newParentId = parentIndex == null ? null : (int?) parentIds[parentIndex.Value];
                        var task = CreateSingleContent(name, newParentId ?? parentId);

                        tasks.Add(key, task);
                    }

                    await Task.WhenAll(tasks.Values);

                    foreach (var (key, value) in tasks)
                    {
                        parentIds[key] = value.Result.Id;
                    }

                    _logger.Information("Created {createdNumber}/{numberInLevel}",
                        Math.Min((j + 1) * _chunkSize, current.Length), current.Length);
                }
            }
        }

        private static IDictionary<int, IList<int>> BuildLevels(uint countOfContentNodes, uint numberOfItemsOnEachLevel)
        {
            var result = new Dictionary<int, IList<int>>();

            for (var i = 0; i < countOfContentNodes; i++)
            {
                var name = GetNameBasedOnPositionInTree((uint) i, numberOfItemsOnEachLevel);
                result[i] = name.Split(new[] { ".", "Item" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                    .ToList();
            }

            return result;
        }

        private static int? GetParentIndex(uint currentItemNumber, uint numberOfItemsOnEachLevel)
        {
            var number = (int) (currentItemNumber / numberOfItemsOnEachLevel);

            return number > 0 ? (int?) number : null;
        }

        private static string GetNameBasedOnPositionInTree(uint currentItemNumber, uint numberOfItemsOnEachLevel)
        {
            var position = string.Empty;
            var runner = currentItemNumber;
            do
            {
                var num = runner % numberOfItemsOnEachLevel;
                runner /= numberOfItemsOnEachLevel;
                position = num + "." + position;
            } while (runner > 0);

            return $"Item {position.Substring(0, position.Length - 1)}";
        }

        private async Task<Content> CreateSingleContent(string name, int parentId = -1)
        {
            var model = new Content
            {
                Id = 0,
                ContentTypeAlias = "testData",
                ParentId = parentId,
                Action = "publishNew",
                Variants = new[]
                {
                    new ContentVariant
                    {
                        Name = name,
                        Properties = new[]
                        {
                            new ContentVariantProperty
                            {
                                Id = 0,
                                Alias = "text",
                                Value = Constants.Data.ShortString
                            }
                        },
                        Culture = "en-US",
                        Publish = true
                    }
                },
                TemplateAlias = "TestData"
            };

            var result = await PostMultipartFormDataAsync(Constants.Paths.CreateContent, model);
            var data = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
            {
                _logger.Error("Create content error: {data}.", data);
            }
            else
            {
                _logger.Verbose("Create Content result: {data}", data);
            }


            _logger.Debug("Creating content: {name}.", name);


            return JsonConvert.DeserializeObject<Content>(data.Substring(Constants.Misc.AngularPrefix.Length));
        }


        private async Task Login()
        {
            var result = await PostJsonAsync(Constants.Paths.Login, new
            {
                username = _user,
                password = _password
            });

            _logger.Verbose("Login result: {result}", result);
            //TODO handle non happy path
        }

        private Task<HttpResponseMessage> PostJsonAsync<T>(string relativePath, T model) =>
            _httpClient.PostAsync(GetUrl(relativePath), GetJsonContent(model));

        private static StringContent GetJsonContent<T>(T model) => new StringContent(JsonConvert.SerializeObject(model,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), Encoding.UTF8, "application/json");

        private Task<HttpResponseMessage> PostMultipartFormDataAsync<T>(string relativePath, T model)
        {
            var content = new MultipartFormDataContent
            {
                { GetJsonContent(model), "contentItem" }
            };

            var response = Policy
                .Handle<TaskCanceledException>()
                .Or<HttpRequestException>()
                .OrResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .WaitAndRetryAsync(10, i => TimeSpan.FromSeconds(2 + i),
                    (result, timeSpan, retryCount, context) =>
                    {
                        _logger.Warning(
                            $"Request failed with {result?.Result?.StatusCode.ToString() ?? "N/A"}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                    })
                .ExecuteAsync(() => _httpClient.PostAsync(GetUrl(relativePath), content));

            return response;
        }


        private string GetUrl(string path) => _backOfficeUrl + path;
    }
}