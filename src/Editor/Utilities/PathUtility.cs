namespace BlinkLab.Editor.Utilities;

public static class PathUtility
{
	public static bool IsValidFileOrDirectoryName(string name)
	{
		char[] invalidPathChars = Path.GetInvalidPathChars();
		char[] invalidFileChars = Path.GetInvalidFileNameChars();
		return !name.Any(c => invalidFileChars.Contains(c) || invalidPathChars.Contains(c));
	}

	public static bool IsValidFileOrDirectoryName(string name, out char[] invalidchars)
	{
		char[] invalidPathChars = Path.GetInvalidPathChars();
		char[] invalidFileChars = Path.GetInvalidFileNameChars();
		bool valid = true;
		List<char> invalidChars = [];

		foreach (char c in name)
		{
			if (invalidPathChars.Contains(c) || invalidFileChars.Contains(c))
			{
				valid = false;
				if (!invalidChars.Contains(c))
				{
					invalidChars.Add(c);
				}
			}
		}

		invalidchars = invalidChars.ToArray();
		return valid;
	}
}
