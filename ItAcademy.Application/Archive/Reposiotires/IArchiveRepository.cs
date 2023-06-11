namespace ItAcademy.Application.Archive.Reposiotires
{
    public interface IArchiveRepository
    {
        Task MoveToArchive(CancellationToken token);
    }
}
