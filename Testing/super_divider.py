import sys

def splitter(value, divider):
    factor_int = int(value / divider)
    remainder = value - (factor_int * divider)
    count = divider - remainder

    print("Output: " + str(count) + "x " + str(factor_int) + " and " + str(remainder) + "x " + str(factor_int + 1))


value = int(sys.argv[1])
divider = int(sys.argv[2])

splitter(value, divider)