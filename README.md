## This application in one click will show the cost of spare parts from different sellers (competitors) and suppliers (NEW)
### The search is carried out by:
- Detail number
- Part name
- Brief description (if such information is available)

### The search result is displayed as product cards:
- Image
- Detail number
- Price
- Part name and brief description
Each card has a link to the seller's product page

![image](https://user-images.githubusercontent.com/103592628/199592823-ccf35f06-3514-4ed2-9dc0-bdf255ad00b3.png)

### Price search results are displayed when you click on the menu button on the top right:
- Detail number
- Part name and brief description
- Price
- Results are grouped by vendors (Supplier name - file name without extension)
![image](https://user-images.githubusercontent.com/103592628/201514384-459e9ebd-a30a-48b1-8aa8-7936191dbae4.png)

### The web application uses the MVC pattern of the ASP.NET
What technologies did you use:
- The application logic is written in C#
- Razor Pages is used to render pages
- For parsing sites Nuget HtmlAgilityPack

The biggest problem is site parsing. Since each site has a different layout, a separate logic was written for each
A particular problem is the last site. Part numbers were indicated there not in a separate line, but in the title.
I wrote a code that separates a string into a name and a number, while calculating the number of opening and closing brackets.
Sometimes the product card did not have an image. Solved it with the default image
When parsing, various kinds of artifacts often appear in the lines, they also had to be calculated separately (maybe something was left)

## What will be changed:
- Combine classes of site product cards and product cards from price lists
- Add sorting of results in ascending order of prices
- Change the layout of the cards so that more information fits on one page without scrolling
- Prices from the price list results will be multiplied by the exchange rate
- Think about how to separate prices by currency: BYN and RUB

### What has already been done
- Add search by supplier prices (local only)
- Improve code readability by adding a single parsing method for all sites
