#!/usr/bin/env bash
# Generates a localhost self-signed cert (localhost.crt and localhost.key)
# Places files in ./certs relative to the todo-app folder.

set -euo pipefail

OUT_DIR="$(dirname "$0")/../certs"
mkdir -p "$OUT_DIR"
pushd "$OUT_DIR" > /dev/null

echo "Creating OpenSSL config for SAN (localhost, 127.0.0.1, ::1)..."
cat > san.cnf <<'EOF'
[ req ]
distinguished_name = req_distinguished_name
req_extensions = v3_req
prompt = no

[ req_distinguished_name ]
CN = localhost

[ v3_req ]
keyUsage = critical, digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names

[ alt_names ]
DNS.1 = localhost
IP.1 = 127.0.0.1
IP.2 = ::1
EOF

echo "Generating private key and certificate (valid 2 years)..."
openssl req -x509 -nodes -newkey rsa:2048 \
  -keyout localhost.key -out localhost.crt -days 730 \
  -config san.cnf -extensions v3_req

chmod 600 localhost.key
echo "Created: $OUT_DIR/localhost.crt and $OUT_DIR/localhost.key"
echo "To trust the cert locally (optional):"
echo "  - macOS: sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain $OUT_DIR/localhost.crt"
echo "  - Linux (Firefox/Chrome may require importing into browser or using system CA store): follow distro-specific steps"

popd > /dev/null

echo "Done. Start dev server with: npm run start:https"
