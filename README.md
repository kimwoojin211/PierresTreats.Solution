# Pierre's Sweet and Savory Treats

#### _Application to help a baker market his treats._

#### By **Woo Jin Kim**

## Technologies Used

* _C#_
* _.NET 5.0.102_
* _ASP.NET Core MVC 4.8_
* _Razor_
* _Entity Framework Core_
* _MySql_
* _Git_
* _Identity_

## Description

_An MVC web application that utilizes a database with many-to-many relationships with user authentication to:_

* Display a splash page that lists all treats and flavors.
* Demonstrate a many-to-many relationship between Treats and Flavors.
* Demonstrate user authentication with log in/out functionality.
* Allow all users to read the database data.
* Allow logged in users to create, update, and delete from the database.

## Setup/Installation Requirements
<br>

_Note: Make sure you have **.NET 5.0** installed. If not, please visit [this link](https://dotnet.microsoft.com/download/dotnet/5.0)_

* _Clone this repository to your computer from [this repository](https://github.com/kimwoojin211/Factory.git)_
* _In your terminal of choice, navigate to the PierresTreats directory_
* _Create a file named "appsettings.json" in this directory_
* _Add the following lines of code to your newly created "appsettings.json" file and save._
```
{
  "ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;database=[YOUR DATABASE];uid=root;pwd=[YOUR PASSWORD];"
  }
}
```
**NOTES:**  _[YOUR PASSWORD]_ **MUST** be the same password as that on your local machine/server.<br>
_[YOUR USERNAME]_ is up to your discretion.<br>
For help on database configuration, please see [here](https://www.learnhowtoprogram.com/c-and-net/getting-started-with-c/installing-and-configuring-mysql)
</details>
<br>

* _Back in the terminal, enter `dotnet build` and ensure that there are no errors_
* _Enter `dotnet ef database update` into the terminal to build the database_ <br>
**(Note: If a "Build failed" Error occurs, please try running `dotnet ef migrations Add Initial` in the terminal. )**
* _Enter `dotnet run` into the terminal to run the application_
<br>

## Known Bugs


## License

_MIT_

## Contact Information

_Woo Jin Kim (kimwoojin211@gmail.com)_
Copyright (c) 2021 Woo Jin Kim
