# Use Node.js v22 as the base image
FROM node:22

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json (if available)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the application
RUN npm run build

# Expose port 3000
EXPOSE 3000

# Start the application using Vite preview on port 3000, binding to all interfaces
CMD ["npm", "run", "preview", "--", "--port", "3000", "--host", "0.0.0.0"]