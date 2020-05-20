$(function () {
    const ko = window.ko;

    function GoalsViewModel(goals, categories) {
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

        self.success = function (amount, goal) {
            return amount >= goal;
        };

        self.fail = function (amount, goal) {
            return !self.success(amount, goal);
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
                    row.actualAmount(response);
                });
        }
    }

    function LimitsViewModel(limits, categories) {
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

        self.success = function (amount, limit) {
            return amount <= limit;
        };

        self.fail = function (amount, limit) {
            return !self.success(amount, limit);
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
                    console.log(response);
                    row.actualAmount(response);
                });
        }
    }

    function init() {
        const model = window.formModel;

        const goalsViewModel = new GoalsViewModel(model.goals, model.categories);
        const limitsViewModel = new LimitsViewModel(model.limits, model.categories);

        ko.applyBindings(goalsViewModel, $("#js-goals")[0]);
        ko.applyBindings(limitsViewModel, $("#js-limits")[0]);
    }

    init();
});