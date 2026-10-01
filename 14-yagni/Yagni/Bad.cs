namespace Yagni.Bad;

// ❌ Requirement: "When a user signs up, send them a welcome e-mail."
//
//    What we built, "to be future-proof":

public interface IMessage { string Body { get; } }
public interface IMessageTemplate<TModel> { IMessage Render(TModel model); }
public interface IMessageChannel { string Name { get; } void Deliver(string recipient, IMessage message); }
public interface IMessageChannelFactory { IMessageChannel Create(string channelName); }
public interface IMessageChannelFactoryProvider { IMessageChannelFactory GetFactory(string tenant); }
public interface IRecipientResolver<TUser> { string Resolve(TUser user, string channelName); }
public interface IMessagingPipelineStep { IMessage Process(IMessage message); }
public interface IWelcomeMessageOrchestrator<TUser> { void Orchestrate(TUser user); }

public record User(string Name, string Email);
public record TextMessage(string Body) : IMessage;

public class WelcomeTemplate : IMessageTemplate<User>
{
    public IMessage Render(User model) => new TextMessage($"Welcome, {model.Name}!");
}

public class EmailChannel : IMessageChannel
{
    public string Name => "email";
    public void Deliver(string recipient, IMessage message) => Console.WriteLine($"[Email] {recipient}: {message.Body}");
}

public class DefaultChannelFactory : IMessageChannelFactory
{
    // Only one channel has ever existed.
    public IMessageChannel Create(string channelName) =>
        channelName == "email" ? new EmailChannel() : throw new NotSupportedException(channelName);
}

public class MultiTenantChannelFactoryProvider : IMessageChannelFactoryProvider
{
    // We have one tenant.
    public IMessageChannelFactory GetFactory(string tenant) => new DefaultChannelFactory();
}

public class UserEmailRecipientResolver : IRecipientResolver<User>
{
    public string Resolve(User user, string channelName) => user.Email;
}

public class NoOpPipelineStep : IMessagingPipelineStep
{
    // "We'll need this for i18n / A-B testing / personalization later."
    public IMessage Process(IMessage message) => message;
}

public class WelcomeMessageOrchestrator<TUser>(
    IMessageTemplate<TUser> template,
    IMessageChannelFactoryProvider providers,
    IRecipientResolver<TUser> resolver,
    IEnumerable<IMessagingPipelineStep> pipeline,
    string tenant = "default",
    string channelName = "email") : IWelcomeMessageOrchestrator<TUser>
{
    public void Orchestrate(TUser user)
    {
        var message = pipeline.Aggregate(template.Render(user), (m, step) => step.Process(m));
        var channel = providers.GetFactory(tenant).Create(channelName);
        channel.Deliver(resolver.Resolve(user, channelName), message);
    }
}
