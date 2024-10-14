using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OpenAI.Tests
{
    public class ChatApiTests
    {
        private OpenAIApi openai = new OpenAIApi();

        [Test]
        public async Task Create_Chat_Completion()
        {
            var req = new CreateChatCompletionRequest
            {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage>()
                {
                    new ChatMessage() { Role = "user", Content = "Hello!" }
                }
            };
            var res = await openai.CreateChatCompletion(req);
            Assert.NotNull(res);
        }

        [Test]
        public async Task Create_Chat_Completion_Stream()
        {
            bool responseReceived = false;
            float timeout = 10;
            float time = 0;

            var req = new CreateChatCompletionRequest
            {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage>()
                {
                    new ChatMessage() { Role = "user", Content = "Hello!" }
                },
                Temperature = 0,
                Stream = true
            };

            openai.CreateChatCompletionAsync(req, null, () => { responseReceived = true; },
                new CancellationTokenSource());

            while (!responseReceived && time < timeout)
            {
                await Task.Delay(100);
                time += 0.1f;
            }

            Assert.IsTrue(responseReceived);
        }

        [Test]
        public async Task Create_Function_Call()
        {
            var toolFunc = new ToolFunction()
            {
                Name = "GetNumber",
                Description = "It will get a number"
            };
            var tool = new Tool()
            {
                Type = "function",
                Function = toolFunc,
            };
            var req = new CreateChatCompletionRequest()
            {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage>()
                {
                    new ChatMessage()
                        { Role = "user", Content = "Hello! Please get me a number. Please just send me a number" }
                },
                Tools = new List<Tool>() { tool }
            };
            var res = await openai.CreateChatCompletion(req);


            Assert.AreEqual(res.Choices[0].Message.ToolCalls[0].Function.Name, "GetNumber",
                "Tool function name is not equal");
        }
    }
}