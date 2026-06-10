## Project Name 
Trainee Management API

## Technology Used
Asp.net core

## How to Run
run `dotnet run` in the root of the project directory

## API List
- GET /api/health
- GET /api/trainees
- GET /api/trainees/{id}
- POST /api/trainees
- PUT /api/trainees/{id}
- DELETE /api/trainees/{id}

## Sample Request JSON 

Sample POST /api/trainees request: 
```javascript
{ 
  "firstName": "john", 
  "lastName": "joe", 
  "email": "john.doe@training.com", 
  "techStack": "HTML, CSS, JavaScript", 
  "status": "Active"
} 
```

Sample PUT /api/trainees/1 request: 
```javascript
{  
  "status": "InActive"
} 
```

## Sample Response JSON
Sample GET /api/health response:
```javascript
{
  "status": "running",
  "application": "Trainee Management App",
  "timestamp": "2026-06-08T11:10:34.0036979+00:00"
}
```

Sample GET /api/trainees response: 
```javascript
[
  {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-08T10:55:05.7288647+00:00",
    "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
  }
]
```

Sample POST /api/trainees response: 
```javascript
{
  "newTrainee": {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-08T10:55:05.7288647+00:00",
    "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
  }
}
```

Sample GET /api/trainees/{id} response: 
```javascript
{
  "id": 1,
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Active",
  "createdDate": "2026-06-08T10:55:05.7288647+00:00",
  "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
}
```

Sample PUT /api/trainees/{id} response:
```javascript
{
  "id": 1,
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Inactive",
  "createdDate": "2026-06-08T10:55:05.7288647+00:00",
  "updatedDate": "2026-06-08T10:57:22.9859447+00:00"
}
```

## Known Limitations 
- Absense of Authentication
- Still using In-memory database instead of Sql or NoSql database

## Database Setup Steps
 
- First import required packages and make sure all the packages are of the same version so that we do not get any version mismatch error.
- Update the Program.cs file for using the MySql database instead of In-memory datase.
- In appsettings.json add another entry for Connection string like this:
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=trainee_management_db;user=root;password=root;"
  },
- Run dotnet build to make sure there are no errors.
- Run the migration command => dotnet ef migrations add InitialCreate
- Once the migration is completed run this command to make tables in the database => dotnet ef database update
- Once ran successfully, the code and the database are in sync. We can test the connection by using swagger UI, try adding one entry using POST end point and see if it is shown in the datase or not.