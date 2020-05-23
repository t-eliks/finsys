$(function () {
    const ko = window.ko;

    function GoalsViewModel(goals, categories) {
        var self = this;

        self.isInitialized = ko.observable(false);

        self.goalArray = ko.observableArray([]);
        self.categoryArray = ko.observableArray(categories);

        goals.forEach(function (goal) {
            self.goalArray.push(ko.viewmodel.fromModel($.extend(goal, {categoryOptions: categories})));
        });

        self.addGoalRow = function () {
            self.goalArray.push(ko.viewmodel.fromModel({
                id: 0,
                goal: '',
                categoryOptions: categories,
                categoryId: 0,
                actualAmount: 0
            }));
        }

        self.success = function (id, amount, goal) {
            return id && amount >= goal;
        };

        self.fail = function (id, amount, goal) {
            return id && !self.success(id, amount, goal);
        };

        self.removeGoal = function (row) {
            $.ajax({
                url: window.removeGoalUrl + '/' + row.id(),
                type: 'DELETE'
            })
                .done(function () {
                    self.goalArray.remove(row);
                });
        }

        self.saveGoal = function (row) {
            if (!row.goal()) {
                return;
            }
            
            $.ajax({
                url: window.saveGoalUrl,
                contentType: 'application/json',
                type: 'POST',
                data: JSON.stringify({
                    Id: row.id(),
                    CategoryId: row.categoryId(),
                    Goal: +row.goal()
                })
            })
                .done(function (response) {
                    row.actualAmount(response.actualAmount);
                    row.id(response.id)
                });
        }
    }

    function LimitsViewModel(limits, categories) {
        var self = this;
        
        self.isInitialized = ko.observable(false);
        
        self.limitArray = ko.observableArray([]);
        self.categoryArray = ko.observableArray(categories);

        limits.forEach(function (limit) {
            self.limitArray.push(ko.viewmodel.fromModel($.extend(limit, {categoryOptions: categories})));
        });

        self.addLimitRow = function () {
            self.limitArray.push(ko.viewmodel.fromModel({
                id: 0,
                limit: '',
                categoryOptions: categories,
                categoryId: 0,
                actualAmount: 0
            }));
        }

        self.success = function (id, amount, limit) {
            return id && amount <= limit;
        };

        self.fail = function (id, amount, limit) {
            return id && !self.success(id, amount, limit);
        };

        self.removeLimit = function (row) {
            $.ajax({
                url: window.removeLimitUrl + '/' + row.id(),
                type: 'DELETE'
            })
                .done(function () {
                    self.limitArray.remove(row);
                });
        }

        self.saveLimit = function (row) {
            if (!row.limit()) {
                return;
            }
            
            $.ajax({
                url: window.saveLimitUrl,
                contentType: 'application/json',
                type: 'POST',
                data: JSON.stringify({
                    Id: row.id(),
                    CategoryId: row.categoryId(),
                    Limit: +row.limit()
                })
            })
                .done(function (response) {
                    row.actualAmount(response.actualAmount);
                    row.id(response.id)
                });
        }
    }

    function init() {
        const model = window.formModel;

        const goalsViewModel = new GoalsViewModel(model.goals, model.categories);
        const limitsViewModel = new LimitsViewModel(model.limits, model.categories);

        ko.applyBindings(goalsViewModel, $("#js-goals")[0]);
        ko.applyBindings(limitsViewModel, $("#js-limits")[0]);
        
        goalsViewModel.isInitialized(true);
        limitsViewModel.isInitialized(true);
    }

    init();
});