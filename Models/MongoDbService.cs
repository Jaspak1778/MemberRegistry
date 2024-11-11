using MongoDB.Bson;
using MongoDB.Driver;


public class MongoDbService
{
    private readonly IMongoCollection<Member> _members;

    public MongoDbService()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("theclub");
        _members = database.GetCollection<Member>("members");
    }

    public async Task<List<Member>> GetAllMembersAsync() =>
        await _members.Find(member => true).ToListAsync();

    public async Task AddMemberAsync(Member member) =>
        await _members.InsertOneAsync(member);
        
    public async Task UpdateMemberAsync(Member member)
    {
        var filter = Builders<Member>.Filter.Eq(m => m.MembershipNumber, member.MembershipNumber);
        var update = Builders<Member>.Update
            .Set(m => m.FirstName, member.FirstName)
            .Set(m => m.LastName, member.LastName)
            .Set(m => m.Address, member.Address)
            .Set(m => m.DateOfBirth, member.DateOfBirth)
            .Set(m => m.Email, member.Email);

        await _members.UpdateOneAsync(filter, update);
    }

    public async Task DeleteMemberAsync(string id) =>
        await _members.DeleteOneAsync(m => m.Id == id);
}
