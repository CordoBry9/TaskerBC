using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TaskerBC.Models;

namespace TaskerBC.Services
{
    public class TaskerItemService
    {

        //dependency injection in C#
        private readonly IJSRuntime _jsRuntime;
        public TaskerItemService(IJSRuntime jsRuntime) //constructor
        { 
            _jsRuntime = jsRuntime;
        }
        public async Task SaveTaskerItems(List<TaskerItem> taskerItems)
        {
            string itemsAsString = JsonSerializer.Serialize(taskerItems); 

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "TaskerItems", itemsAsString); 
        }

        public async Task<List<TaskerItem>> GetTaskerItems()
        {
            List<TaskerItem> taskerItems = new List<TaskerItem>();

            try
            {
                string? tasksAsJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "TaskerItems"); //<> type parameters

                if (!string.IsNullOrEmpty(tasksAsJson))
                {
                    taskerItems = JsonSerializer.Deserialize<List<TaskerItem>>(tasksAsJson)!;

                }
            }
            catch (Exception ex)
            {
                await SaveTaskerItems(taskerItems);

                Console.WriteLine("Cold not load TaskerItems from local storage {0}", ex);
            }

            return taskerItems;
        }
    }


}
