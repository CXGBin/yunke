namespace YunKeEdu.Core.Exceptions;

public class BizException : Exception
{
    public int Code { get; }

    public BizException(string message, int code = 400) : base(message)
    {
        Code = code;
    }
}
