# henry-api
A reservations api for the Henry coding assessment

## Prerequisites

This project requires the following prerequisites in order to be built and run locally:

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Running the project locally

In order to run this project locally you should take the following steps.

### From a bash terminal:

1. Ensure you have trusted the dotnet dev certs for https
    `dotnet dev-certs https --trust`

2. Clone the project locally
   `git clone https://github.com/WesleyWaffles/henry-api.git`

3. Navigate to the project
    `cd henry-api`

4. Build the solution and execute unit tests
    `dotnet build && dotnet test`

5. Run the Henry.Api project
    `dotnet run --project src/Henry.Api/ -lp https`

6. Navigate to `https://localhost:7281/swagger/` in a browser to view the swagger docs and interact with the api

### Navigating the project structure

In the project you'll find that the code in `Program.cs` is sparse. This is the complaint I've seen most often about minimal apis. Insted you'll find that the routes are mapped in the endpoint classes in the `v0` folder, that being the version of the api.

I also tried to scope the majority of the business logic to the appointment service at `Services\AppointmentService.cs` to make it a little easier to wrangle that logic and test it.

You'll find that there is a unit tests project at `tests\Henry.Api.UnitTests` with a few unit tests for the core rules provided in the assesment instructions. This project makes use of the entity framework in memory database. Typically I would prefer to just integration test with a real database, but I thought this was a good way to keep everything local and useable.

## Notes for my reviewers

Thank you for taking the time to review this code and provide feedback. I took this assessment as an opportunity to explore the dotnet minimal api features. I don't really write a lot of apis, and haven't tackled one in years, but I've been interested in dotnet's newer minimal api features for a while now, and specifically how we might structure projects with it after watching some talks about it and poking through the asp.net examples. 

I tried to demonstrate a good project structure and clean code, but I'm sure you'll have ample opporunity for questions and criticism, and to be honest I'm looking forward to that. 

Lastly on the timing - the instructions were fairly specific about trying to keep my time to about two hours. I don't want to misrepresent myself, that simply wasn't enough time for me to get there. You can see my commit history in the repository from over the weekend, and I had a few bursts of time at an hour here or there. I'd say in total I'm just over four hours of active time. I don't really mind, as I said this was a great opportunity for me to dig into minimal api anyways, but if that time portion is critical you can checkout the repo at commit `b2d556e`. That represents roughly the two hour mark.
