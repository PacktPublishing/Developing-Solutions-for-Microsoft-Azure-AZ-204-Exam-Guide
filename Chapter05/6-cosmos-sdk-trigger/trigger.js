function validateOrder() {
    var theContext = getContext();
    var theRequest = theContext.getRequest();   
    // item going to be created 
    var item = theRequest.getBody(); 
    // validate properties
    if (item["OrderCustomer"] != undefined && item["OrderCustomer"] != null) { 
        // update the item that will be created
        theRequest.setBody(item); 
    }else
    {
        //cancel operation
        throw new Error('OrderCustomer must be specified'); 
    }

}
