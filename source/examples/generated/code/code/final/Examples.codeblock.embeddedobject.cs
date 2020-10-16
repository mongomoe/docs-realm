
// create a User (a user is an assignee)
var myUser = new User {
    Name = "Test User 001"
};

realm.Write(() =>
{
    realm.Add(myUser);
});

// create a Task that has an embedded Assignee 

var taskItem = new Task
{
    Name = "Go Jogging",
    Assignee = myUser

};

realm.Write(() =>
{
    realm.Add(taskItem);
});
