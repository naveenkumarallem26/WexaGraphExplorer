# ------------------------------------------------------------
# Stage 1: Build Angular application
# ------------------------------------------------------------

FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build


# ------------------------------------------------------------
# Stage 2: Run Angular SSR application
# ------------------------------------------------------------

FROM node:22-alpine AS final

WORKDIR /app

COPY --from=build /app/dist/WexaGraphExplorer.Web ./dist/WexaGraphExplorer.Web

ENV NODE_ENV=production

ENV PORT=10000

EXPOSE 10000

CMD ["node", "dist/WexaGraphExplorer.Web/server/server.mjs"]
