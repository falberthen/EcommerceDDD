
module.exports = {
  preset: 'jest-preset-angular',
  moduleDirectories: [ "node_modules", "src"],

  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testEnvironment: "jsdom",
  moduleNameMapper: {
    "@core/(.*)": "<rootDir>/src/app/core/$1",
    "@ecommerce/(.*)": "<rootDir>/src/app/modules/ecommerce/$1",
    "@authentication/(.*)": "<rootDir>/src/app/modules/authentication/$1",
    "@shared/(.*)": "<rootDir>/src/app/shared/$1",
    "@environments/(.*)": "<rootDir>/src/environments/$1"
  },
  coveragePathIgnorePatterns: ["/models","/constants",  "/environments"]
};