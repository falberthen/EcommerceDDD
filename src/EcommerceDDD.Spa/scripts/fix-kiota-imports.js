const fs = require('fs');
const path = require('path');

function walk(dir, cb) {
  for (const file of fs.readdirSync(dir)) {
    const full = path.join(dir, file);
    if (fs.statSync(full).isDirectory()) {
      walk(full, cb);
    } else if (full.endsWith('.ts')) {
      cb(full);
    }
  }
}

function stripJsExtensions(file) {
  const content = fs.readFileSync(file, 'utf8');
  const fixed = content.replace(/(from\s+['"].+)\.js(['"])/g, '$1$2');
  if (content !== fixed) {
    fs.writeFileSync(file, fixed);
    console.log(`Fixed imports in ${file}`);
  }
}

walk('src/app/clients', stripJsExtensions);
