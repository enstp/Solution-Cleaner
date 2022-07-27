namespace SolutionCleaner.Core.Services.Contracts
{
	public interface ILocalStorage
	{
		void WriteString(string name, string value);

		void DeleteItem(string name);

		bool ItemExists(string name);

		string ReadString(string name);
	}
}