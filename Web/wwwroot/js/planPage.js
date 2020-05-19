$(function() {
    const ko = window.ko;
    
    function GoalsViewModel(goals, categories) {
        self.goalArray = ko.observableArray([]);
        self.categoryArray = ko.observableArray(categories);
        
        goals.forEach(function (goal) {
            self.goalArray.push(ko.viewmodel.fromModel($.extend(goal, { categoryOptions: categories })));
        });
        
        self.addGoalRow = function () {
            self.goalArray.push(ko.viewmodel.fromModel({
                id: 0,
                limit: '',
                categoryOptions: categories,
                categoryId: 0
            }));
        }

        self.remove = function(row) {
            self.goalArray.remove(row);
        }
        
        self.save = function(row) {
            $.ajax({
                url: window.saveGoalUrl,
                contentType: 'application/json',
                type: 'POST',
                data: JSON.stringify({
                    Id: row.id(),
                    CategoryId: row.categoryId(),
                    Limit: +row.limit()
                })
            })
                .done(function (response) {
                    console.log('Saved');
                });
        }
    }
    
    function init() {
        const model = window.formModel;
        console.log(model);
        const goalsViewModel = new GoalsViewModel(model.goals, model.categories);
        
        ko.applyBindings(goalsViewModel, $("#js-goals")[0]);
    }
    
    init();
});