namespace Yagni.Good;

// ✅ What was actually asked for.
//    When SMS arrives (if ever), THEN introduce an abstraction - shaped by
//    the real second use case instead of an imagined one.

public record User(string Name, string Email);

public class WelcomeEmail
{
    public void SendTo(User user) => Console.WriteLine($"[Email] {user.Email}: Welcome, {user.Name}!");
}
