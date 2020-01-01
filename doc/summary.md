# BookStore

# Objects

## Anonymous Users

> All users who initially connect to our website are in this class and maintain in it until they log in with an account. They can search for books online, view book information for specific books, add books to their shopping carts, modify books in their shopping cart, view help menus, create new user accounts, and login to the system.

## Logged in Users

> Once users have logged in from a previously created account, they have all the rights and functionality of an anonymous user as well as the ability to proceed to the checkout. Once there, they can confirm and then place their order.

## Pages for anonymous users


1. Home Page 
2. Search Page 
3. Book Information Page 
4. Account Creation Page 
5. Shopping Basket 
6. Login Page 
7. Help/Information Page 
8. F.A.Q. 
9. System Rules Page 
10. Contact Page The logged in user can view also view: 
11. Checkout Page

## Bar

* Home
* Search
* Create Account
* Login
* About US/Help
* Shopping Basket
* Checkout

# Home page

* Welcome page
* route to 
  * create new accout
  * login

## Search Page

* Search by
  * author 
  * title 
  * ISBN number

> None case sensitive text

* query exp
  * Author
  * title
  * isbn

## Book Information Page

| Quantity in Stock | Availability message displayed    |
| ----------------- | --------------------------------- |
| <0                | Usually ships within 4 weeks      |
| 0-4               | Usually ships within 1 week       |
| 5-19              | Usually ships within 2 to 3 days  |
| >=20              | Usually ships with in 1 to 2 days |

## Account Creation Page

* username
* password
* go to 
  * automatically logged in 
  * directed to their shopping basket

## Shopping Basket

* modify the quantity
* delete book
* update
* invalid quantity deiny
* cooike

## Login Page

* Action path
  * Forgot password
    * E-mail message to administrator
  * Login: Incorrect
    * return this page
  * Login: correct
    * create cookie
    * redirect to shopping basket

## Help/Information page

* F.A.Q
* Rules
* contact information

## Checkout page

> Only when a user has at least one item in their shopping basket and has logged into our system will they be able to proceed to the checkout page. First, they will be asked to confirm the items in their order. Next, they will be asked to provide a valid credit card for the order as well as shown their shipping address for the order. Finally, they will be shown the information for their entire order and asked to confirm it. Once they confirm the order, it is sent to the database for processing.  Even if quantities of a specific book are not available at the time of purchase, the transaction will still be completed. If a book’s quantity in stock falls to below zero, that will represent to the store owners that X number of holds have been placed on the book, to be filled as soon as new copies of the book arrives.  The application layer will present multiple forms to the user each asking them a small set of questions including asking them for their credit card information and whether they would like to confirm their order or not. Each form will submit to a JSP, which will then generate a new form based on the results of the previous form. When all the information has been processed and confirmed, the order will be added to the database and the user will be informed.  In addition, the quantities of the books in stock will be decremented by the quantities of books the user purchased for each book. 

