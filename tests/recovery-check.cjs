const {request}=require('@playwright/test');
(async()=>{
 const client=await request.newContext({baseURL:'http://127.0.0.1:8082'});
 const token=(await(await client.get('/api/auth/csrf')).json()).token;
 const login=await client.post('/api/auth/login',{headers:{'X-CSRF-TOKEN':token},data:{username:'secretaria',password:'CrecheCad-Demo-2026!'}});
 if(login.status()!==200)throw new Error('Administrator rescue did not restore login');
 const data=await(await client.get('/api/dashboard')).json();
 if(data.students!==12||data.classes!==3||data.teachers!==3)throw new Error('Records were lost during recovery');
 console.log('Administrator recovery and persistent school records verified.');await client.dispose();
})().catch(error=>{console.error(error);process.exitCode=1;});
