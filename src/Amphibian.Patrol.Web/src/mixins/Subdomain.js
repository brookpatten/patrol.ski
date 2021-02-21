export default {
    methods: {
    },
    computed: {
      subdomain: function () {
        var host = window.location.host;
        if(host.indexOf(':')>=0){
            var portParts = host.split(':');
            host = portParts[0];
            var port = portParts[1];
        }
        var hostParts = host.split('.');

        var subdomain = "";
        if(hostParts.length>2){
            for(var i=0;i<hostParts.length-2;i++){
                if(subdomain.length>0){
                    subdomain = subdomain+".";
                }
                subdomain = subdomain+hostParts[i];
            }
        }
        else if(hostParts.length>1 && hostParts[hostParts.length-1]=="localhost")
        {
            for(var i=0;i<hostParts.length-1;i++){
                if(subdomain.length>0){
                    subdomain = subdomain+".";
                }
                subdomain = subdomain+hostParts[i];
            }
        }

        if(subdomain!="www"){
            return subdomain;
        }
        else{
            return null;
        }
    }
  }
}