namespace Discord.Interactions;

public static class GenericInputModal
{
    public static string GetGenericIdWord(int number)
    {
        return number switch
        {
            0 => "first",
            1 => "second",
            2 => "third",
            3 => "fourth",
            4 => "fifth",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static string GetNextCustomId(ModalBuilder modal)
    {
        int count = modal.Components.ActionRows.Count;
        return GetGenericIdWord(count);
    }
}

public interface IGenericInputModal : IModal
{

}

public class GenericInputModal<T1> : IGenericInputModal
{
    public string Title => string.Empty;

    [ModalTextInput("first")]
    public T1 First { get; set; }
}

public class GenericInputModal<T1, T2> : GenericInputModal<T1>
{
    [ModalTextInput("second")]
    public T2 Second { get; set; }
}

public class GenericInputModal<T1, T2, T3> : GenericInputModal<T1, T2>
{
    [ModalTextInput("third")]
    public T3 Third { get; set; }
}

public class GenericInputModal<T1, T2, T3, T4> : GenericInputModal<T1, T2, T3>
{
    [ModalTextInput("fourth")]
    public T4 Fourth { get; set; }
}

public class GenericInputModal<T1, T2, T3, T4, T5> : GenericInputModal<T1, T2, T3, T4>
{
    [ModalTextInput("fifth")]
    public T5 Fifth { get; set; }
}
