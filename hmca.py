import hmac
import hashlib
import sys



shared_secret = b+sys.argv[2]
sequence = sys.argv[1];
body = sys.argv[3]

# Create HMAC
hmac_calculator = hmac.new(shared_secret, digestmod=hashlib.sha256)
hmac_calculator.update(sequence.encode())
hmac_calculator.update(b' ')
hmac_calculator.update(body.encode())

# Print HMAC in lowercase
calculated_hmac = hmac_calculator.hexdigest()
print(f'Calculated HMAC: {calculated_hmac}')
