module.exports = {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testEnvironment: 'jsdom',
  testPathIgnorePatterns: ['<rootDir>/node_modules/', '<rootDir>/dist/'],
  coverageDirectory: 'coverage',
  transform: {
    '^.+\\.(ts|mjs|js|html)$': [
      'jest-preset-angular',
      {
        tsconfig: '<rootDir>/tsconfig.spec.json',
        stringifyContentPathRegex: '\\.(html|svg)$',
      },
    ],
  },
  transformIgnorePatterns: ['node_modules/(?!(@microsoft|@opentelemetry|tinyduration|@angular|ngx-toastr|@ng-bootstrap|@fortawesome)/)'],
  moduleFileExtensions: ['ts', 'html', 'js', 'json', 'mjs'],
  moduleNameMapper: {
    '^zone\\.js$': '<rootDir>/node_modules/zone.js/fesm2015/zone.js',
    '^(.*)\\.js$': '$1',
    '^src/app/clients/(.*)$': '<rootDir>/src/app/clients/$1',
    '@core/(.*)': '<rootDir>/src/app/core/$1',
    '@features/ecommerce/(.*)': '<rootDir>/src/app/features/ecommerce/$1',
    '@features/authentication/(.*)': '<rootDir>/src/app/features/authentication/$1',
    '@shared/(.*)': '<rootDir>/src/app/shared/$1',
    '@environments/(.*)': '<rootDir>/src/environments/$1',
  },
  coveragePathIgnorePatterns: ['/models', '/constants', '/environments'],
};
