using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;

public class ChatAi : MonoBehaviour
{
    private OpenAIApi openAI = new OpenAIApi("sk-proj-XpJetuWlQU1R0rUmYMNGT3BlbkFJAhRl7ym2M6Ze4Nxkwiyv", "org-573aTAZXMQ5zToWIdFniMsCm");
    private List<ChatMessage> messages = new List<ChatMessage>();

    public async void TalkToTeacher(string newTextInput)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newTextInput;
        newMessage.Role = "user"; 

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";


        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count>0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}