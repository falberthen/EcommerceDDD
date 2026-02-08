module.exports = {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testPathIgnorePatterns: [
    '<rootDir>/node_modules/',
    '<rootDir>/dist/'
  ],
  coverageDirectory: 'coverage',
  transform: {
    '^.+\\.(ts|mjs|js|html)$': 'jest-preset-angular',
  },
  transformIgnorePatterns: ['node_modules/(?!(@microsoft|@opentelemetry|tinyduration|jest-preset-angular)/)'],
  snapshotSerializers: [
    'jest-preset-angular/build/serializers/no-ng-attributes',
    'jest-preset-angular/build/serializers/ng-snapshot',
    'jest-preset-angular/build/serializers/html-comment',
  ],
  moduleNameMapper: {
    "@core/(.*)": "<rootDir>/src/app/core/$1",
    "@ecommerce/(.*)": "<rootDir>/src/app/modules/ecommerce/$1",
    "@authentication/(.*)": "<rootDir>/src/app/modules/authentication/$1",
    "@shared/(.*)": "<rootDir>/src/app/shared/$1",
    "@environments/(.*)": "<rootDir>/src/environments/$1"
  },
  coveragePathIgnorePatterns: ["/models", "/constants", "/environments"]
};
