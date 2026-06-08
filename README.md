## Project Name 
Trainee Management API

## Technology Used
Asp.net core

## How to Run
run `dotnet run` in the root of the project directory

## API List
- GET /api/trainees
- GET /api/trainees/{id}
- POST /api/trainees
- PUT /api/trainees/{id}
- DELETE /api/trainees/{id}

## Sample Request JSON 
Sample POST /api/trainees request: 
{ 
  "firstName": "john", 
  "lastName": "joe", 
  "email": "john.doe@training.com", 
  "techStack": "HTML, CSS, JavaScript", 
  "status": "Active"
} 

Sample PUT /api/trainees/1 request: 
{  
  "status": "InActive"
} 

## Sample Response JSON
Sample GET /api/trainees response: 
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

Sample POST /api/trainees response: 
{
  "newTrainee": {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active"
  }
}

Sample GET /api/trainees/{id} response: 
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

Sample PUT /api/trainees/{id} response: 
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

## Known Limitations 
- Absense of Authentication
- Still using In-memory database instead of Sql or NoSql database