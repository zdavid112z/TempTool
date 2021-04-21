module.exports = {
    apps: [{
        name: 'Backend',
        script: 'cd /www/backend && python3 -m swagger_server',
        watch: true,
        autorestart: true,
        kill_timeout : 5000,
        env: {}
    }]
}
