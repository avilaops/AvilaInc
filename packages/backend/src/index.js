"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.app = void 0;
const express_1 = __importDefault(require("express"));
const core_1 = require("@vizzio/core");
const app = (0, express_1.default)();
exports.app = app;
app.get('/health', (_req, res) => {
    res.json({ status: 'ok', timestamp: new Date().toISOString() });
});
app.get('/panel', (_req, res) => {
    res.json({
        automationCatalog: core_1.automationCatalog,
        automationCapabilities: core_1.automationCapabilities,
        devtoolsBlueprint: core_1.devtoolsBlueprint,
        devtoolsChecklist: core_1.devtoolsChecklist,
    });
});
app.get('/panel/summary', (_req, res) => {
    res.json({
        capabilities: core_1.automationCapabilities.length,
        categories: core_1.automationCatalog.length,
        blueprintSections: core_1.devtoolsBlueprint.length,
        checklistItems: core_1.devtoolsChecklist.length,
    });
});
exports.default = app;
//# sourceMappingURL=index.js.map