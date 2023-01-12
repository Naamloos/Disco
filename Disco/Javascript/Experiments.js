webpackChunkdiscord_app.push([["wp_isdev_patch"], {}, r => cache = Object.values(r.c)]);
var UserStore = cache.find(m => m?.exports?.default?.getCurrentUser).exports.default;
var actions = UserStore._dispatcher._actionHandlers._orderedActionHandlers["CONNECTION_OPEN"];
var user = UserStore.getCurrentUser();
actions.find(n => n.name === "ExperimentStore").actionHandler({
	type: "CONNECTION_OPEN", user: { flags: user.flags |= 1 }, experiments: [],
});
actions.find(n => n.name === "DeveloperExperimentStore").actionHandler();
webpackChunkdiscord_app.pop(); user.flags &= ~1; "done";