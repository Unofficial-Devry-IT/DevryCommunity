try:
    # if user types in something that's not a number
    # if will throw a ValueError Exception
    a = int(input("Enter a number: "))
except ValueError as e:
    # catching ValueError exception and storing the exception
    # as the variable 'e'
    print("Not a number:\t\t", e)
