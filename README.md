 I Created Entites, Migrations ,Controller, ServiceContract ,DTO,Services
 ON Entities I create Models regarding the Entity Table
 ON Migration I created Addtable, AddPasswordSalt And Seed Cateogry Migrations
 on ServiceContract I create DTO for data transfer related operations , and Interface 
 on Service I Implement the Interfaces 
 on controller I create version V1 folder and create UserController , Product controller
 --------------------------------------------------------------------------------------------------
APIs:-
-------------------------------------------User--------------------------------------------------------------------
------------------------------------
Post: /api/v{version}/User/register
input the version 1
and set the followring fields values:-
{
  "username": "string",
  "passwordHash": "string",
  "passwordSalt": "string",
  "roleID": 0
} 
***Role ID will be 1 from Administrator and 2 for User
---------------------------------------------
Get: /api/v{version}/User/login
enter the User Name and Password and version 
--------------------------------------Product---------------------------------------------------------------------------
----------------------------------------
 Post: /api/v{version}/Product/add
Enter the version name and fill the following fields:-
{
  "name": "string",
  "description": "string",
  "price": 0,
  "sku": "string",
  "createdDate": "2024-11-10T16:51:25.971Z",
  "productCategories": [0]
}
**on productCategories enter the products categories sepreate from comma like 1,2 which will be already on Cateogry table
--------------------------------------
Get: /api/v{version}/Product
Get All products using enter the version number
---------------------------------------------
Get:/api/v{version}/Product/{productId}
Enter ProductID and VersionID then get that particular product details
-------------------------------------------------
Get: /api/v{version}/Product/category/{categoryId}
Enter CategoryID and Version No then get the All products which will be related to that category
-----------------------------------------------
Put: /api/v{version}/Product/{ProductID}
Enter Product ID ,version no and further fields
{
  "name": "string",
  "description": "string",
  "price": 0,
  "sku": "string",
  "categoryIDs": [0]
}
for update the particular product details
-----------------------------------------------
Post:/api/v{version}/Product/{productId}/adjust-price
ENter product ID ,versionno, Adjustment Amount , if you want to adjust according to percentage select isPercentage true or else false
-------------------------------------------------------------------------------------------------------------------------------------
Update the Funtioanlity of Product 
work on Inventory 
Maintain Error handling, sirilog,AND JWT authentication Bearer and Apply the documentation on API for how to use the API 
create  Unit test for user and product
 
