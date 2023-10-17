import datetime


# ANSI escape codes for text colors
class Colors:
    RED = "\033[31m"
    GREEN = "\033[32m"
    YELLOW = "\033[33m"
    BLUE = "\033[34m"
    CYAN = "\033[1;36m"
    RESET = "\033[0m"


def print_colored(message, color):
    print(color + message + Colors.RESET)


def gcd_euclidean(x, y):
    """
    Compute the GCD of 2 numbers using the Euclidean algorithm
    """
    if x == 0 or x == y:  # Check if x is 0 or x is equal to y, return y (GCD is y in this case)
        return y
    elif y == 0:  # Else check if y is 0, return x (GCD is x in this case)
        return x
    while y > 0:  # Use a while loop to repeatedly apply the Euclidean algorithm
        temp = x % y  # Calculate the remainder of x divided by y
        x = y  # Update x to be y
        y = temp  # Update y to be the remainder (temp)
    return x  # Return the final value of x, which is the GCD


def gcd_prime_factors(x, y):
    """
    Compute the GCD of 2 numbers by decomposing the numbers into products of prime factors
    The GCD is the product of the common factors, taken at the lowest power.
    """
    if x == 0 or x == y:  # Check if x is 0 or x is equal to y, return y (GCD is y in this case)
        return y
    elif y == 0:   # Check if y is 0, return x (GCD is x in this case)
        return x
    i = 2  # Initialize i to 2 (the first prime number)
    greatest_common_divisor = 1  # Initialize the GCD as 1
    while x > i or y > i:  # Use a while loop to iterate through potential prime factors
        while x % i == 0 and y % i == 0:  # Check if both x and y are divisible by the current prime factor (i)
            greatest_common_divisor *= i  # If both are divisible, update the GCD by multiplying it with the prime factor
            x //= i
            y //= i  # Divide x and y by the prime factor
        i += 1  # Move on to the next prime factor
    return greatest_common_divisor  # Return the final value of the GCD, which is the product of common prime factors


def gcd_brute_force(x, y):
    """
    Consider the GCD as being the smallest number between the given two and subtract from this number until it divides
    both numbers
    """
    if x == 0 or x == y:  # Check if x is 0 or x is equal to y, return y (GCD is y in this case)
        return y
    elif y == 0:  # Check if y is 0, return x (GCD is x in this case)
        return x
    greatest_common_divisor = min(x, y)   # Initialize the GCD as the smaller of the two numbers
    while x % greatest_common_divisor != 0 or y % greatest_common_divisor != 0:  # Use a while loop to find the GCD by subtracting from the smallest number
        greatest_common_divisor -= 1   # If the current GCD doesn't divide both x and y, subtract 1 from it
    return greatest_common_divisor  # Return the final value of the GCD


if __name__ == '__main__':
    tests = [
        (18, 12),
        (30, 17),
        (45, 70),
        (255, 300),
        (255, 177),
        (101, 301),
        (4137524, 1227244),
        # (294733, 10383680172),
        (4 ** 4, 6 ** 4),
        # (2 ** 50, 4 ** 20),
        (6 ** 3, 3 ** 7),
        # (12345665434562, 57658787675842),
        (182364, 116033),
        (70004, 43602),
        (10662, 78376)
    ]
    for test in tests:
        first = test[0]
        second = test[1]
        print_colored(f"\nx={first},y={second}", Colors.CYAN)  # Using cyan color for x and y

        print("Start Euclidean GCD")
        start = datetime.datetime.now()
        gcd = gcd_euclidean(first, second)
        end = datetime.datetime.now()
        print("Time elapsed: {}".format(end - start))
        print("Gcd is {}\n".format(gcd))

        print("Start Prime Factorization GCD")
        start = datetime.datetime.now()
        gcd = gcd_prime_factors(first, second)
        end = datetime.datetime.now()
        print("Time elapsed: {}".format(end - start))
        print("Gcd is {}\n".format(gcd))

        print("Start Brute Force GCD")
        start = datetime.datetime.now()
        gcd = gcd_brute_force(first, second)
        end = datetime.datetime.now()
        print("Time elapsed: {}".format(end - start))
        print("Gcd is {}\n".format(gcd))