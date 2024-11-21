# Final Project for PT Berca Hardayaperkasa Bootcamp (BECARE : Berca Reimbursement)
-----------------------------------------------------

## Our Member
- Edo Laksana Widodo
- Ikhsan Kurniawan
- Muhammad Yusuf Syahidin

## Dependencies
this project is using .Net 6.0 and .Net SDK 8.0.403 as a backend (API), so here are some dependencies you need to install:
- BCrypt.Net-Next v4.0.3
- Microsoft.AspNetCore.Authentication.JwtBearer v6,0,35
- Microsoft.EntityFrameworkCore v7.0.20
- Microsoft.EntityFrameworkCore.SqlServer v7.0.20
- Microsoft.EntityFrameworkCore.Tools v7.0.20

also this project is using Vue.Js as a frontend (Client)

## Things you need to do to run the project
- open appsetting.json on the API and change the connectionstring base on empty database you have and your sql server credentials
- try to run the code, it should show you a swagger ui on the new tab/windows of your browser
- if there is no problem running the code, now open the client
- run this syntax on the terminal `npm install` to instal some required dependencies/library
- type `npm run dev` to run the code, and go to the link when you run the command
- cuz we dont include account seeder to this program so you need to input one account to the program manually first, open the AccountController and TitleController and go to `AddAccount` and `AddTitle` on the API and comment or remove this line `[Authorize(Roles = "HR")]` from the code and save then rerun the API
- input this to the `POST /api/Title` endpoint :
```
{
  "id_Title": "string",     <== this one just keep it like this, cuz actually this doesnt change/input anything but if you dont include this, it will gives you an error, dunno why
  "title_Name": "Employee",
  "reimburse_Limit": 7000000
}
```
- input this to the `POST /api/Account` endpoint : 
```
{
  "email": "hr@email.com",
  "role_Name": "HR",
  "id_Title": "T001",
  "name": "HRD",
  "phone": "089876543210",
  "gender": "Male",
  "birth_Date": "2024-11-21",
  "join_Date": "2024-11-21"
}
```
- change back what you have done to the AccountController and TitleController.
- try open the login page and try login using credential you just created, and the default password is `12345`
- happy coding!
