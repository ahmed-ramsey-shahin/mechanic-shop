using System.Text.Json.Serialization;

namespace MechanicShop.Domain.Common.Results;

public readonly record struct Success;
public readonly record struct Deleted;
public readonly record struct Created;
public readonly record struct Updated;

public static class Result
{
    public static Success Success => default;
    public static Deleted Deleted => default;
    public static Created Created => default;
    public static Updated Updated => default;
}

public sealed class Result<TValue> : IResult<TValue>
{
    private readonly TValue? _value;
    private readonly List<Error>? _errors;
    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;
    public List<Error> Errors => IsError ? _errors! : [];
    public TValue Value => IsSuccess ? _value! : default!;
    public Error TopError => (_errors?.Count > 0) ? _errors[0] : default;

#pragma warning disable IDE0051
    [JsonConstructor]
    private Result(TValue? value, List<Error>? errors, bool isSucess)
    {
        IsSuccess = isSucess;
        if (IsSuccess)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _errors = [];
        }
        else
        {
            if (errors == null || errors.Count == 0)
            {
                throw new ArgumentException("Can not create an Error<TValue> from an empty collection of errors.", nameof(errors));
            }
            _errors = errors;
            _value = default;
        }
    }
#pragma warning restore IDE0051

    private Result(Error error)
    {
        _errors = [error];
        IsSuccess = false;
    }

    private Result(List<Error> errors)
    {
        if (errors is null || errors.Count == 0)
        {
            throw new ArgumentException("Can not create an Error<TValue> from an empty collection of errors.", nameof(errors));
        }
        _errors = errors;
        IsSuccess = false;
    }

    private Result(TValue value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        IsSuccess = true;
        _value = value;
    }

    public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onValue, Func<List<Error>, TNextValue> onError) => IsSuccess ? onValue(Value) : onError(Errors);

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);
}
