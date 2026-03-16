#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P_APPRO2_ZweiteVieApp.Data;
using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.PageModels
{
    public partial class ProjectListPageModel : ObservableObject
    {
        private readonly ProjectRepository _projectRepository;

        [ObservableProperty]
        private List<Project> _projects = [];

        public ProjectListPageModel(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            Projects = await _projectRepository.ListAsync();
        }

        [RelayCommand]
        Task NavigateToProject(Project project)
            => Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        async Task AddProject()
        {
            await Shell.Current.GoToAsync($"project");
        }
    }
}