namespace Microservice.HelloWorld;

public class PersonHandler
{
    public Task<PersonModel> CreatePerson(PersonInputModel person)
    {
        return Task.FromResult(new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = "Kake"
        });
    }

    public Task<PersonModel> GetPerson(Guid id)
    {
        return Task.FromResult(new PersonModel
        {
            Id = id,
            Name = "Kake"
        });
    }
}
