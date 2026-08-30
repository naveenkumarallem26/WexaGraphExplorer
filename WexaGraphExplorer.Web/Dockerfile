FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build


FROM node:22-alpine AS final

WORKDIR /app

COPY --from=build /app/dist/WexaGraphExplorer.Web ./dist/WexaGraphExplorer.Web

ENV PORT=4000

EXPOSE 4000

CMD ["node", "dist/WexaGraphExplorer.Web/server/server.mjs"]
