using CommunityToolkit.Mvvm.Input;
using P_APPRO2_ZweiteVieApp.Models;

namespace P_APPRO2_ZweiteVieApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}