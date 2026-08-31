#!/usr/bin/env node

import { spawnSync } from "node:child_process";
import { createHash } from "node:crypto";
import {
  copyFileSync,
  existsSync,
  mkdirSync,
  readFileSync,
  readdirSync,
  renameSync,
  rmSync,
  statSync,
  writeFileSync,
} from "node:fs";
import path from "node:path";

const RESTORE_REQUIRED_EXIT_CODE = 42;
const MANIFEST_NAME = "canvaskit.manifest.json";
const STAMP_SCHEMA = "doroti.canvaskit-restore-stamp/v1";
const MANIFEST_SCHEMA = "doroti.canvaskit-assets/v1";

class ValidationError extends Error {}

function fail(message) {
  throw new ValidationError(message);
}

function parseArguments(argv) {
  const [command, ...rest] = argv;
  if (command !== "check" && command !== "prepare") {
    fail("The first argument must be 'check' or 'prepare'.");
  }

  const values = new Map();
  for (let index = 0; index < rest.length; index += 2) {
    const key = rest[index];
    const value = rest[index + 1];
    if (!key?.startsWith("--") || value === undefined) {
      fail(`Invalid argument sequence near '${key ?? "<end>"}'.`);
    }

    if (values.has(key)) {
      fail(`Argument '${key}' was supplied more than once.`);
    }

    values.set(key, value);
  }

  const required = ["--web-root", "--pin", "--stamp", "--npm"];
  if (command === "prepare") {
    required.push("--intermediate-root", "--output-root", "--logical-base-path");
  }

  for (const key of required) {
    if (!values.has(key)) {
      fail(`Required argument '${key}' is missing.`);
    }
  }

  return { command, values };
}

function readJson(filePath, description) {
  let text;
  try {
    text = readFileSync(filePath, "utf8");
  } catch (error) {
    fail(`${description} is missing or unreadable at '${filePath}': ${error.message}`);
  }

  try {
    return JSON.parse(text);
  } catch (error) {
    fail(`${description} is not valid JSON at '${filePath}': ${error.message}`);
  }
}

function sha256File(filePath) {
  const hash = createHash("sha256");
  hash.update(readFileSync(filePath));
  return hash.digest("hex");
}

function compareVersions(actual, minimum, toolName) {
  const parse = (value) => {
    const match = /^(\d+)\.(\d+)\.(\d+)/u.exec(value.trim().replace(/^v/u, ""));
    if (!match) {
      fail(`Unable to parse ${toolName} version '${value}'.`);
    }

    return match.slice(1).map(Number);
  };

  const actualParts = parse(actual);
  const minimumParts = parse(minimum);
  for (let index = 0; index < 3; index += 1) {
    if (actualParts[index] > minimumParts[index]) {
      return;
    }

    if (actualParts[index] < minimumParts[index]) {
      fail(`${toolName} ${minimum}+ is required; found ${actual}.`);
    }
  }
}

function validateToolchain(pin, npmExecutable) {
  compareVersions(process.versions.node, pin.minimumNodeVersion, "Node.js");

  let npmResult;
  if (process.platform === "win32") {
    if (/[\r\n"&|<>^]/u.test(npmExecutable)) {
      fail(`npm executable '${npmExecutable}' contains unsupported command characters.`);
    }

    const quotedExecutable = npmExecutable.includes(" ") ? `"${npmExecutable}"` : npmExecutable;
    npmResult = spawnSync(process.env.ComSpec ?? "cmd.exe", ["/d", "/s", "/c", `${quotedExecutable} --version`], {
      encoding: "utf8",
      windowsHide: true,
    });
  } else {
    npmResult = spawnSync(npmExecutable, ["--version"], {
      encoding: "utf8",
    });
  }
  if (npmResult.error || npmResult.status !== 0) {
    const detail = npmResult.error?.message ?? npmResult.stderr?.trim() ?? `exit ${npmResult.status}`;
    fail(`npm is unavailable: ${detail}`);
  }

  compareVersions(npmResult.stdout.trim(), pin.minimumNpmVersion, "npm");
}

function validatePin(pin) {
  if (pin.schema !== "doroti.canvaskit-pin/v1") {
    fail(`Unsupported CanvasKit pin schema '${pin.schema ?? "<missing>"}'.`);
  }

  if (pin.packageName !== "canvaskit-wasm" || pin.version !== "0.42.0" || pin.variant !== "default") {
    fail("CanvasKit must remain pinned to canvaskit-wasm@0.42.0, default variant.");
  }

  if (
    !pin.integrity?.startsWith("sha512-") ||
    !pin.resolved?.startsWith("https://registry.npmjs.org/") ||
    !/^[0-9a-f]{64}$/u.test(pin.manifestSha256 ?? "")
  ) {
    fail("The CanvasKit pin must use the approved npm registry URL and SHA-512 integrity.");
  }

  const expectedTargets = ["canvaskit.js", "canvaskit.wasm", "types/index.d.ts", "LICENSE"];
  if (!Array.isArray(pin.files) || pin.files.length !== expectedTargets.length) {
    fail("The CanvasKit source allowlist must contain exactly four files.");
  }

  const actualTargets = pin.files.map((file) => file.target);
  if (actualTargets.some((target, index) => target !== expectedTargets[index])) {
    fail(`The CanvasKit target allowlist must be exactly: ${expectedTargets.join(", ")}.`);
  }

  for (const file of pin.files) {
    if (
      typeof file.source !== "string" ||
      path.isAbsolute(file.source) ||
      file.source.split(/[\\/]/u).includes("..") ||
      /(^|[\\/])(full|profiling)([\\/]|$)/u.test(file.source)
    ) {
      fail(`CanvasKit source '${file.source}' is outside the approved default variant allowlist.`);
    }

    if (!Number.isSafeInteger(file.byteLength) || file.byteLength <= 0 || !/^[0-9a-f]{64}$/u.test(file.sha256)) {
      fail(`CanvasKit pin metadata is invalid for '${file.target}'.`);
    }
  }
}

function validatePackageAndLock(webRoot, pinPath) {
  const packagePath = path.join(webRoot, "package.json");
  const lockPath = path.join(webRoot, "package-lock.json");
  const pin = readJson(pinPath, "CanvasKit pin");
  validatePin(pin);

  const packageJson = readJson(packagePath, "Web package manifest");
  const packageDependencyKeys = Object.keys(packageJson.devDependencies ?? {});
  if (
    packageDependencyKeys.length !== 1 ||
    packageDependencyKeys[0] !== pin.packageName ||
    packageJson.devDependencies[pin.packageName] !== pin.version
  ) {
    fail(`package.json must have exactly one exact devDependency: ${pin.packageName}@${pin.version}.`);
  }

  if (packageJson.dependencies !== undefined || packageJson.scripts !== undefined) {
    fail("The CanvasKit asset package must not add runtime dependencies or npm scripts.");
  }

  if (
    packageJson.engines?.node !== `>=${pin.minimumNodeVersion}` ||
    packageJson.engines?.npm !== `>=${pin.minimumNpmVersion}`
  ) {
    fail("package.json toolchain engines do not match the approved CanvasKit pin.");
  }

  const lock = readJson(lockPath, "Web package lockfile");
  if (lock.lockfileVersion !== 3) {
    fail(`package-lock.json must use lockfileVersion 3; found '${lock.lockfileVersion ?? "<missing>"}'.`);
  }

  const rootPackage = lock.packages?.[""];
  if (rootPackage?.devDependencies?.[pin.packageName] !== pin.version) {
    fail("package-lock.json root dependency does not match the exact CanvasKit pin.");
  }

  const lockedPackage = lock.packages?.[`node_modules/${pin.packageName}`];
  if (
    lockedPackage?.version !== pin.version ||
    lockedPackage?.resolved !== pin.resolved ||
    lockedPackage?.integrity !== pin.integrity
  ) {
    fail("package-lock.json CanvasKit version, registry URL, or integrity does not match the approved pin.");
  }

  return {
    lockPath,
    packagePath,
    pin,
    hashes: {
      lockfileSha256: sha256File(lockPath),
      packageJsonSha256: sha256File(packagePath),
      pinSha256: sha256File(pinPath),
    },
  };
}

function expectedStamp(inputs) {
  return {
    schema: STAMP_SCHEMA,
    packageName: inputs.pin.packageName,
    version: inputs.pin.version,
    variant: inputs.pin.variant,
    ...inputs.hashes,
  };
}

function validateInstalledPackage(webRoot, pin, missingMeansRestore) {
  const packageRoot = path.join(webRoot, "node_modules", pin.packageName);
  const installedPackagePath = path.join(packageRoot, "package.json");
  if (!existsSync(installedPackagePath)) {
    if (missingMeansRestore) {
      return { restoreRequired: true };
    }

    fail(`Installed ${pin.packageName} package is missing at '${packageRoot}'.`);
  }

  const installedPackage = readJson(installedPackagePath, "Installed CanvasKit package manifest");
  if (installedPackage.name !== pin.packageName || installedPackage.version !== pin.version) {
    fail(`Installed CanvasKit package is not ${pin.packageName}@${pin.version}.`);
  }

  for (const file of pin.files) {
    const sourcePath = path.resolve(packageRoot, file.source);
    const relativeSource = path.relative(packageRoot, sourcePath);
    if (relativeSource.startsWith("..") || path.isAbsolute(relativeSource)) {
      fail(`CanvasKit source '${file.source}' escapes the installed package root.`);
    }

    if (!existsSync(sourcePath)) {
      if (missingMeansRestore) {
        return { restoreRequired: true };
      }

      fail(`Required CanvasKit source asset '${file.source}' is missing.`);
    }

    const actualLength = statSync(sourcePath).size;
    const actualHash = sha256File(sourcePath);
    if (actualLength !== file.byteLength || actualHash !== file.sha256) {
      fail(
        `CanvasKit source asset '${file.source}' failed its approved byte length/SHA-256 check ` +
          `(expected ${file.byteLength}/${file.sha256}, got ${actualLength}/${actualHash}).`,
      );
    }
  }

  return { packageRoot, restoreRequired: false };
}

function isSameJsonValue(left, right) {
  return JSON.stringify(left) === JSON.stringify(right);
}

function checkRestore(webRoot, stampPath, inputs) {
  const installed = validateInstalledPackage(webRoot, inputs.pin, true);
  if (installed.restoreRequired || !existsSync(stampPath)) {
    return true;
  }

  let actualStamp;
  try {
    actualStamp = readJson(stampPath, "CanvasKit restore stamp");
  } catch (error) {
    if (error instanceof ValidationError) {
      return true;
    }

    throw error;
  }

  return !isSameJsonValue(actualStamp, expectedStamp(inputs));
}

function listFiles(root, prefix = "") {
  if (!existsSync(root)) {
    return [];
  }

  const files = [];
  for (const entry of readdirSync(root, { withFileTypes: true })) {
    const relative = prefix ? `${prefix}/${entry.name}` : entry.name;
    const fullPath = path.join(root, entry.name);
    if (entry.isDirectory()) {
      files.push(...listFiles(fullPath, relative));
    } else {
      files.push(relative);
    }
  }

  return files.sort();
}

function assertSafeOutput(outputRoot, intermediateRoot, webRoot) {
  const outputRelative = path.relative(intermediateRoot, outputRoot);
  if (!outputRelative || outputRelative.startsWith("..") || path.isAbsolute(outputRelative)) {
    fail(`CanvasKit output '${outputRoot}' must be a child of target intermediate root '${intermediateRoot}'.`);
  }

  const webRelative = path.relative(webRoot, outputRoot);
  if (!webRelative.startsWith("..") && !path.isAbsolute(webRelative)) {
    fail(`CanvasKit output '${outputRoot}' must not write into the Web source tree.`);
  }
}

function atomicCopy(source, destination) {
  mkdirSync(path.dirname(destination), { recursive: true });
  const temporary = `${destination}.tmp-${process.pid}`;
  try {
    copyFileSync(source, temporary);
    rmSync(destination, { force: true });
    renameSync(temporary, destination);
  } finally {
    rmSync(temporary, { force: true });
  }
}

function atomicWriteText(destination, contents) {
  mkdirSync(path.dirname(destination), { recursive: true });
  const temporary = `${destination}.tmp-${process.pid}`;
  try {
    writeFileSync(temporary, contents, { encoding: "utf8" });
    rmSync(destination, { force: true });
    renameSync(temporary, destination);
  } finally {
    rmSync(temporary, { force: true });
  }
}

function prepareAssets(webRoot, stampPath, intermediateRoot, outputRoot, logicalBasePath, inputs) {
  assertSafeOutput(outputRoot, intermediateRoot, webRoot);
  const installed = validateInstalledPackage(webRoot, inputs.pin, false);
  const allowedOutputFiles = new Set([...inputs.pin.files.map((file) => file.target), MANIFEST_NAME]);
  const unexpected = listFiles(outputRoot).filter((file) => !allowedOutputFiles.has(file));
  if (unexpected.length > 0) {
    fail(`CanvasKit intermediate output contains non-allowlisted files: ${unexpected.join(", ")}.`);
  }

  for (const file of inputs.pin.files) {
    atomicCopy(path.resolve(installed.packageRoot, file.source), path.resolve(outputRoot, file.target));
  }

  const logicalBase = logicalBasePath.endsWith("/") ? logicalBasePath : `${logicalBasePath}/`;
  const manifest = {
    schema: MANIFEST_SCHEMA,
    packageName: inputs.pin.packageName,
    version: inputs.pin.version,
    variant: inputs.pin.variant,
    registryTarball: inputs.pin.resolved,
    lockfileIntegrity: inputs.pin.integrity,
    lockfileSha256: inputs.hashes.lockfileSha256,
    packageJsonSha256: inputs.hashes.packageJsonSha256,
    logicalBasePath: logicalBase,
    canvasKitJsPath: "canvaskit.js",
    canvasKitWasmPath: "canvaskit.wasm",
    canvasKitJsUrl: `${logicalBase}canvaskit.js`,
    canvasKitWasmUrl: `${logicalBase}canvaskit.wasm`,
    files: inputs.pin.files.map((file) => ({
      path: file.target,
      byteLength: file.byteLength,
      sha256: file.sha256,
    })),
  };
  const manifestPath = path.join(outputRoot, MANIFEST_NAME);
  atomicWriteText(manifestPath, `${JSON.stringify(manifest, null, 2)}\n`);
  const manifestHash = sha256File(manifestPath);
  if (manifestHash !== inputs.pin.manifestSha256) {
    fail(
      `Generated CanvasKit provenance manifest hash changed ` +
        `(expected ${inputs.pin.manifestSha256}, got ${manifestHash}). Update the approved pin and consumer validator together.`,
    );
  }
  atomicWriteText(stampPath, `${JSON.stringify(expectedStamp(inputs), null, 2)}\n`);

  const actualOutputFiles = listFiles(outputRoot);
  const expectedOutputFiles = [...allowedOutputFiles].sort();
  if (!isSameJsonValue(actualOutputFiles, expectedOutputFiles)) {
    fail(
      `CanvasKit intermediate output does not match the exact allowlist. ` +
        `Expected ${expectedOutputFiles.join(", ")}; got ${actualOutputFiles.join(", ")}.`,
    );
  }

  for (const file of inputs.pin.files) {
    const copiedPath = path.join(outputRoot, file.target);
    const copiedLength = statSync(copiedPath).size;
    const copiedHash = sha256File(copiedPath);
    if (copiedLength !== file.byteLength || copiedHash !== file.sha256) {
      fail(`Copied CanvasKit asset '${file.target}' failed its byte length/SHA-256 check.`);
    }
  }
}

function main() {
  const { command, values } = parseArguments(process.argv.slice(2));
  const webRoot = path.resolve(values.get("--web-root"));
  const pinPath = path.resolve(values.get("--pin"));
  const stampPath = path.resolve(values.get("--stamp"));
  const inputs = validatePackageAndLock(webRoot, pinPath);
  validateToolchain(inputs.pin, values.get("--npm"));

  if (command === "check") {
    if (checkRestore(webRoot, stampPath, inputs)) {
      console.log("CanvasKit npm restore is required.");
      process.exitCode = RESTORE_REQUIRED_EXIT_CODE;
      return;
    }

    console.log(`CanvasKit npm restore is current (${inputs.pin.packageName}@${inputs.pin.version}).`);
    return;
  }

  prepareAssets(
    webRoot,
    stampPath,
    path.resolve(values.get("--intermediate-root")),
    path.resolve(values.get("--output-root")),
    values.get("--logical-base-path"),
    inputs,
  );
  console.log(`Prepared verified CanvasKit ${inputs.pin.version} default assets at '${values.get("--output-root")}'.`);
}

try {
  main();
} catch (error) {
  if (error instanceof ValidationError) {
    console.error(`CanvasKit asset validation failed: ${error.message}`);
    process.exitCode = 1;
  } else {
    throw error;
  }
}
