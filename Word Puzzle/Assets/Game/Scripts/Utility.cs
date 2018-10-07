using System;

public static class Utility {

	public static string Shuffle(string str)
	{
		char[] array = str.ToCharArray();
		Random rnd = new Random();
		int n = array.Length;
		while (n > 1)
		{
			n--;
			int k = rnd.Next(n + 1);
			var value = array[k];
			array[k] = array[n];
			array[n] = value;
		}

		return new string(array);
	}

}