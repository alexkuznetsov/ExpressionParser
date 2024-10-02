namespace ExpressionParser.WebApiExample.Common;


public interface IMessage<T> : MediatR.IRequest<T>
{
}

public interface ICommand<T> : IMessage<T>
{
}

public interface ICommandHandler<TCommand, TResult> : MediatR.IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{

}


public interface IMessageHandler<TMessage, TResult> : MediatR.IRequestHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{

}