const express = require('express');
const cors = require('cors');
const { Sequelize, DataTypes } = require('sequelize');

const app = express();
app.use(cors());
app.use(express.json());

app.use(express.static('public')); 

const sequelize = new Sequelize('oop_lab4_db', 'postgres', '14881488', {
    host: 'localhost',
    dialect: 'postgres',
    logging: false
});

const Sensor = sequelize.define('Sensor', {
    magnitudeType: { type: DataTypes.INTEGER, allowNull: false },
    minRange: { type: DataTypes.FLOAT, allowNull: false },
    maxRange: { type: DataTypes.FLOAT, allowNull: false },
    currentValue: { type: DataTypes.FLOAT, allowNull: false }
});

const Device = sequelize.define('Device', {
    locationNumber: { type: DataTypes.INTEGER, allowNull: false },
    calibrationDate: { type: DataTypes.DATEONLY, allowNull: false }
});

const Channel = sequelize.define('Channel', {
    name: { type: DataTypes.STRING, allowNull: true }
});

Channel.hasMany(Device, { as: 'devices', onDelete: 'CASCADE' });
Device.belongsTo(Channel);
Device.belongsTo(Sensor);

const Participant = sequelize.define('Participant', {
    firstName: { type: DataTypes.STRING, allowNull: false },
    lastName: { type: DataTypes.STRING, allowNull: false },
    birthDate: { type: DataTypes.DATEONLY, allowNull: false }
});

const Performance = sequelize.define('Performance', {
    isTeam: { type: DataTypes.BOOLEAN, allowNull: false },
    resultScore: { type: DataTypes.FLOAT, allowNull: false }
});

const Competition = sequelize.define('Competition', {
    name: { type: DataTypes.STRING, allowNull: false }
});

Competition.hasMany(Performance, { as: 'performances', onDelete: 'CASCADE' });
Performance.belongsTo(Competition);
Performance.belongsTo(Participant);

sequelize.sync({ alter: true }).then(() => {
    console.log("PostgreSQL: Всі таблиці створено та оновлено!");
});

app.get('/api/task1/channels', async (req, res) => {
    try {
        const channels = await Channel.findAll({
            include: [{ model: Device, as: 'devices' }]
        });
        const dtoList = channels.map(ch => ({
            id: ch.id,
            name: ch.name,
            devicesCount: ch.devices.length,
            shortInfo: `Вимірювальний канал №${ch.id} (${ch.name || 'Без назви'})`
        }));
        res.json(dtoList);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/task1/channels/:id', async (req, res) => {
    try {
        const channel = await Channel.findByPk(req.params.id, {
            include: [{ model: Device, as: 'devices', include: [Sensor] }]
        });
        if (!channel) return res.status(404).json({ error: "Канал не знайдено" });
        res.json(channel);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task1/channels', async (req, res) => {
    try { res.json(await Channel.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.put('/api/task1/channels/:id', async (req, res) => {
    try {
        await Channel.update(req.body, { where: { id: req.params.id } });
        res.json({ success: true });
    } catch (err) { res.status(400).json({ error: err.message }); }
});

app.get('/api/task1/sensors', async (req, res) => {
    try { res.json(await Sensor.findAll()); } 
    catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task1/sensors', async (req, res) => {
    try { res.json(await Sensor.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.get('/api/task1/devices', async (req, res) => {
    try { res.json(await Device.findAll({ include: [Sensor, Channel] })); } 
    catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task1/devices', async (req, res) => {
    try { res.json(await Device.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.put('/api/task1/devices/:id', async (req, res) => {
    try {
        await Device.update(req.body, { where: { id: req.params.id } });
        res.json({ success: true });
    } catch (err) { res.status(400).json({ error: err.message }); }
});

app.get('/api/task2/competitions', async (req, res) => {
    try {
        const competitions = await Competition.findAll({
            include: [{ model: Performance, as: 'performances', include: [Participant] }]
        });
        
        const dtoList = competitions.map(comp => {
            let winnerSurname = "Немає виступів";
            if (comp.performances && comp.performances.length > 0) {
                const best = comp.performances.reduce((p, c) => (p.resultScore > c.resultScore) ? p : c);
                winnerSurname = best.Participant ? best.Participant.lastName : "Невідомий";
            }
            return {
                id: comp.id,
                name: comp.name,
                shortInfo: `${comp.name} | Переможець: ${winnerSurname}`
            };
        });
        res.json(dtoList);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/task2/competitions/:id', async (req, res) => {
    try {
        const comp = await Competition.findByPk(req.params.id, {
            include: [{ model: Performance, as: 'performances', include: [Participant] }]
        });
        if (!comp) return res.status(404).json({ error: "Змагання не знайдено" });
        res.json(comp);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task2/competitions', async (req, res) => {
    try { res.json(await Competition.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.put('/api/task2/competitions/:id', async (req, res) => {
    try {
        await Competition.update(req.body, { where: { id: req.params.id } });
        res.json({ success: true });
    } catch (err) { res.status(400).json({ error: err.message }); }
});

app.get('/api/task2/participants', async (req, res) => {
    try { res.json(await Participant.findAll()); } 
    catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task2/participants', async (req, res) => {
    try { res.json(await Participant.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.get('/api/task2/performances', async (req, res) => {
    try { res.json(await Performance.findAll({ include: [Competition, Participant] })); } 
    catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/task2/performances', async (req, res) => {
    try { res.json(await Performance.create(req.body)); } 
    catch (err) { res.status(400).json({ error: err.message }); }
});

app.put('/api/task2/performances/:id', async (req, res) => {
    try {
        await Performance.update(req.body, { where: { id: req.params.id } });
        res.json({ success: true });
    } catch (err) { res.status(400).json({ error: err.message }); }
});

const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Node.js Сервер працює на http://localhost:${PORT}`);
});