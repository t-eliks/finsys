$(function() {
    const ko = window.ko;
    
    function ExpenseFormViewModel(model) {
        self.viewModel = ko.viewmodel.fromModel(model);
        self.categoryOptions = ko.observableArray(model.availableCategories);
        self.errorMessage = ko.observable('');
        
        self.save = function() {
            var data = JSON.stringify({
                Amount: self.viewModel.amount() !== null ? +self.viewModel.amount() : null,
                Id: +self.viewModel.id(),
                Comment: self.viewModel.comment(),
                Origin: self.viewModel.origin(),
                CategoryId: self.viewModel.categoryId()
            });
            
            $.ajax({
                url: window.saveUrl,
                contentType: 'application/json',
                type: 'POST',
                data: data
            })
                .done(function (response) {
                    location.replace('/expenses');
                })
                .fail(function (error) {
                    self.errorMessage(error.responseText);
                });
        }
    }
    
    (function init() {
        const model = window.formModel;
        const viewModel = new ExpenseFormViewModel(model);

        ko.applyBindings(viewModel, $("#js-expense-form")[0]);
        ko.applyBindings(viewModel, $("#js-error-message")[0]);
    })()
});