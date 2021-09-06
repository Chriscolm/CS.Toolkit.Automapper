namespace CS.Toolkit.Automapper.Contracts
{
    public interface IObjectMapper
    {
        TOut Map<TIn, TOut>(TIn source) where TIn: new() where TOut: new();
    }
}
