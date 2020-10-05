"""
    Here you will notice we have specified parameters
    In addition, you'll see we specified "default" values 
    for each argument. That means if we do not have to 
    provide a value as seen below
"""

def log(message="This is a message", other="Some other stuff"):
    """
        the f in front of the quotation marks allows us to 
        inject arguments into the string. Based on your IDE 
        your arguments may show up a different color while 
        inside the string
    """
    print(f"\t{message}\n\t{other}")

def log_2(message="This is a message", other="Some other stuff"):
    """
        You can use the Curly brackets {}  - to say "this 
        is where I want to insert an argument"... it goes 
        in order. So message in this case gets put in the 
        first slot, other gets put in the second.
    """

    print("\t{}\n\t{}".format(message, other))

log()
log_2()
print("\n\n")
log("Meow Meow")
log_2("Never gonna", "Give you up")