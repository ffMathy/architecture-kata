Console.WriteLine("=== BAD: subtypes that break the base type's promises ===");
LiskovSubstitution.Bad.Client.ResizeToBanner(new LiskovSubstitution.Bad.Rectangle());
LiskovSubstitution.Bad.Client.ResizeToBanner(new LiskovSubstitution.Bad.Square());
LiskovSubstitution.Bad.Client.Backup(new LiskovSubstitution.Bad.FileStore());
LiskovSubstitution.Bad.Client.Backup(new LiskovSubstitution.Bad.ReadOnlyArchiveStore());

Console.WriteLine();
Console.WriteLine("=== GOOD: every subtype is substitutable for its abstraction ===");
LiskovSubstitution.Good.Client.PrintArea(new LiskovSubstitution.Good.Rectangle(10, 2));
LiskovSubstitution.Good.Client.PrintArea(new LiskovSubstitution.Good.Square(4));
LiskovSubstitution.Good.Client.Backup(new LiskovSubstitution.Good.FileStore());
// LiskovSubstitution.Good.Client.Backup(new LiskovSubstitution.Good.ReadOnlyArchiveStore(...));
//   ^ does not compile - the type system caught what used to be a runtime crash.
