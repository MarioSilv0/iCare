
const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target: "https://localhost:7266",
    secure: false
  },
  {
    context: [
      "/taco",
    ],
    "target": "http://localhost:4000",
    "secure": false,
    "pathRewrite": {
      "^/taco": "/graphql"
    },
    "changeOrigin": true,
    "logLevel": "debug"
  },
  {
    context: [
      "/themealdb",
    ],
    target: "https://www.themealdb.com",
    secure: false,
    "pathRewrite": {
      "^/themealdb": "/api/json/v1/1"
    },
    "changeOrigin": true,
    "logLevel": "debug"
  },
  {
    context: [
      "/translate",
    ],
    target: "http://127.0.0.1:5000",
    secure: false,
    "changeOrigin": true,
    "logLevel": "debug"
  },
]

module.exports = PROXY_CONFIG;
