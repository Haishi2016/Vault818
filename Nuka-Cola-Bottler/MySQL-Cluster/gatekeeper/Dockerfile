FROM node:boron

WORKDIR /usr/src/app
COPY package.json .
RUn npm install
COPY server.js .

EXPOSE 8180

CMD ["npm", "start"]
