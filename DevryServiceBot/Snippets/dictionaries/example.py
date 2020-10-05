"""
    The curly brackets below is how we can 
    initialize a dictionary
"""

inventory = { }

"""
    What if we want to have predefined 
    keys/values in our dictionary?
"""
sword = {
    "description": "Excellent for stabbing things",
    "stackable": False,
    "count": 1
}

health_potion = {
    "description": "Accidently stab yourself with that sword? Here ya go ya goof",
    "stackable": True,
    "count": 5
}


"""
    To add a value, in Python all we have to do is use the following

    We can technically give it any value we want... another dictionary... a name, number
    Only thing is... you'll want to have some sort of known structure otherwise... it'll
    get out of hand quickly

    Notice... when printed it looks A LOT LIKE JSON. Because it pretty much IS JSON.
"""

inventory["sword"] = sword
inventory["health_potion"] = health_potion

print(inventory)

def is_stackable(item):
    return item["stackable"]

def is_stackable_2(item):
    """
    We can also use the dictionary.get function when trying to get a value
    The second parameter is the 'default' value given if the key does not exist
    in the dictionary
    """
    return item.get("stackable", False)     

print(is_stackable(inventory["sword"]))
print(is_stackable(inventory["health_potion"]))
print(is_stackable_2(inventory.get("hopes and dreams", {})))   

"""
    Below is how you can remove a specific item from a dictionary
"""
del inventory["sword"]
print(inventory)